using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using RevKitt.KS.KineticEnvironment;
using RevKitt.KS.KineticEnvironment.JSConverters;

namespace KineticUI
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SimService
    {
        [OperationContract]
        [WebGet]
        public void AddStart(string patternName, double startTime, int id, int priority)
        {
            State.ProgramSim.PatternProvider.AddPattern(patternName, (int) (startTime*1000), id, priority);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void RemoveStart(int id)
        {
            State.ProgramSim.PatternProvider.RemovePattern(id);
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public Stream GetStarts()
        {
            return Serializer.ToStream(State.ProgramSim.PatternProvider.PatternStarts);
        }

        [OperationContract]
        [WebGet]
        public void ShiftAfter(double shiftAt, double amountToShift)
        {
            State.ProgramSim.PatternProvider.ShiftAfter((int) (shiftAt*1000), (int) (amountToShift*1000));
            Saver.LocalSave();
        }

        [OperationContract]
        [WebGet]
        public void SetFalloff(double falloff)
        {
            State.Plugin.FallOff = falloff;
        }

        [OperationContract]
        [WebGet]
        public double GetFalloff()
        {
            return State.Plugin.FallOff;
        }

        [OperationContract]
        [WebGet]
        public void SetSpeed(double speed)
        {
            State.ProgramSim.Speed = speed;
            State.GenSim.Speed = speed;
        }

        [OperationContract]
        [WebGet]
        public double GetSpeed()
        {
            return State.ProgramSim.Speed;
        }

        [OperationContract]
        [WebGet]
        public bool IsKinectAttached()
        {
            return State.Plugin.HasKinect;
        }

        [OperationContract]
        [WebGet]
        public bool IsKinectEnabled()
        {
            return State.Plugin.Enabled;
        }

        [OperationContract]
        [WebGet]
        public String SetKinectEnabled(bool enable)
        {
            if (!State.Plugin.HasKinect)
                return "Kinect is not attached, please connect it to your computer";
            State.Plugin.Enabled = enable;

            //update the model on the kinect...
            //todo: your code sucks joseph
            foreach (var node in State.ProgramSim.Nodes)
            {
                var position = node.Position;

                State.Plugin.XMax = Math.Max(State.Plugin.XMax, position.X);
                State.Plugin.XMin = Math.Min(State.Plugin.XMin, position.X);
                State.Plugin.YMax = Math.Max(State.Plugin.YMax, position.Y);
                State.Plugin.YMin = Math.Min(State.Plugin.YMin, position.Y);
            }

            if (State.Plugin.Enabled)
                return "Kinect Enabled";
            return "Kinect Disabled";
        }


    }
}
