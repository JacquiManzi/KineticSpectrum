using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using KineticUI.JSConverters;

namespace KineticUI
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SimService
    {
        [OperationContract]
        [WebGet]
        public void AddStart(string patternName, double startTime, int id)
        {
            State.Simulation.AddPattern(patternName, (int) (startTime*1000), id);
        }

        [OperationContract]
        [WebGet]
        public void RemoveStart(int id)
        {
            State.Simulation.RemovePattern(id);
        }

        [OperationContract]
        [WebGet]
        public Stream GetStarts()
        {
            return Serializer.ToStream(State.Simulation.PatternStarts);
        }

        [OperationContract]
        [WebGet]
        public void ShiftAfter(double shiftAt, double amountToShift)
        {
            State.Simulation.ShiftAfter((int) (shiftAt*1000), (int) (amountToShift*1000));
        }

    }
}
