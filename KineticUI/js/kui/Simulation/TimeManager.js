define([
        "dojo/_base/declare", 
        "dojo/_base/array",
        "kui/ajax/SimState",
        "dojo/Deferred"
    ], function(declare, array, SimState, Deferred) {
        var INTERVAL_SECONDS = 5;
        var OLD_TIME = 15;
        var Status = { Pending: "PENDING", Empty: "EMPTY", Present:"PRESENT"};


        var deferredNoOp = function(value) {
            var deferred = new Deferred();
            deferred.resolve(value);
            return deferred;
        };

        var RangeState = function(start, end) {
            this.start = start;
            this.end = end;
            this.status = Status.Empty;
            this.states = null;

            this.load = function() {
                if (this.status !== Status.Empty) {
                    return deferredNoOp();
                }
                var deferred = new Deferred();
                this.states = [];
                this.times = [];

                this.status = Status.Pending;
                var worker = new Worker('/js/kui/Simulation/LoadWorker.js');
                worker.onmessage = dojo.hitch(this, this._loadSuccess, deferred);
                worker.onerror = dojo.hitch(this, this._loadError, deferred);
                worker.postMessage({ start: this.start, end: this.end, url:document.URL });

                return deferred;
            };

            this._loadSuccess = function(deferred, worker) {
                var rangeState = worker.data;
                if (!rangeState.done) {
                    this.states.push(rangeState.state);
                    this.times.push(rangeState.time);
                }
                if ((this.times.length > 20 || rangeState.done)
                       && this.status !== Status.Present) {
                    this.status = Status.Present;
                    deferred.resolve();
                }
            };

            this._loadError = function(deferred, worker, err) {
                this.status = Status.Empty;
                this.states = null;
                deferred.reject(err, true);
            };
            
            this.getState = function(now) {
                if (!this.isNow(now)) {
                    throw new Error("Attempting to get state for an invalid time");
                }
                if (this.status === Status.Empty) {
                    //if not present attempt to load and return with delay
                    var deferred = this.load().then(dojo.hitch(this, this.getState));
                    if (!this.states || !this.times) {
                        return deferred;
                    }
                }
                var index = this.times.binarySearch(now);
                return deferredNoOp(this.states[index]);
            },

            this.isNow = function(now) {
                return now >= this.start && now < this.end;
            };
            this.timeTillStart = function(now, simEnd) {
                //should be running now
                if (this.start <= now && this.end > now) {
                    return 0;
                }
                //will be running in this loop
                if (this.start > now) {
                    return this.start - now;
                }
                //already ran, time till run in next loop
                if (this.end <= now) {
                    return this.start + (simEnd - now);
                }
                throw Error("Unexpected state...");
            };

            this.clearIfOld = function(now, simEnd) {
                if (this.timeTillStart(now, simEnd) > OLD_TIME) {
                    this.state = null;
                    this.status = Status.Empty;
                }
            };
        };
        return declare("kui.Simulation.TimeManager", null, {

            rangeStates: null,
            endTime: 0,
            
            constructor: function() {
                this.rangeStates = [];
            },
            
            newSimulation: function(endTime) {
                this.rangeStates = [];
                this.endTime = endTime;
                for (var i = 0; i < endTime / INTERVAL_SECONDS; i++) {
                    var start = i * INTERVAL_SECONDS;
                    var end = Math.min(endTime, (i + 1) * INTERVAL_SECONDS);
                    this.rangeStates.push(new RangeState(start, end));
                }
                if (this.rangeStates.length > 0) {
                    return this._updateRange(this.rangeStates[0]);
                }
                return deferredNoOp();
            },

            getState: function(now) {
                if (this.endTime == 0) {
                    return deferredNoOp([]);
                }
                var index = Math.floor(now / INTERVAL_SECONDS);
                var deferred = this.rangeStates[index].getState(now);
                var nextIndex = (index + 1) % this.rangeStates.length;
                this._updateRange(this.rangeStates[nextIndex]);
                //stupid workaround to javascripts inability to correctly mod negative numbers
                var prevIndex = (index - 1 + this.rangeStates.length) % this.rangeStates.length;
                this.rangeStates[prevIndex].clearIfOld(now, this.endTime);

                return deferred;
            },

            _updateRange: function(rangeState) {
                var index = Math.round(rangeState.start / INTERVAL_SECONDS);
                return rangeState.load()
                    .then(dojo.hitch(this, function() {
                        index = (index + 1) % this.rangeStates.length;
                        if (index < this.rangeStates.length) {
                            var nextState = this.rangeStates[index];
                            if (nextState.status === Status.Empty &&
                                nextState.timeTillStart < OLD_TIME) {
                                this._updateRange(nextState);
                            }
                        }
                    }));
            }
        }); 
    });