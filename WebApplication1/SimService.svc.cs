using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SimService
    {

        [OperationContract]
        [WebGet]
        public int AddStart(string patternName, double startTime)
        {
            return State.Simulation.AddPattern(patternName, (int) (startTime*1000)).Id;
        }

        [OperationContract]
        [WebGet]
        public void RemoveStart(int id)
        {
            State.Simulation.RemovePattern(id);
        }

        [OperationContract]
        [WebGet]
        public void ShiftAfter(double shiftAt, double amountToShift)
        {
            State.Simulation.ShiftAfter((int) (shiftAt*1000), (int) (amountToShift*1000));
        }

    }
}
