using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STK_demonstration.Som
{
    public class SatUserParameter : RACoN.RTILayer.CHlaInteractionClass
    {
        #region Declarations
        public RACoN.RTILayer.CHlaInteractionParameter SatUserName;
        public RACoN.RTILayer.CHlaInteractionParameter SatUserAltitude;
        public RACoN.RTILayer.CHlaInteractionParameter SatUserOrbitalInclination;
        public RACoN.RTILayer.CHlaInteractionParameter SatUserRightAscensionOfAscendingNode;
        #endregion
        public SatUserParameter() : base()
        {
            // Initialize Class Properties
            this.ClassName = "InteractionRoot.SatUserParameter";
            this.ClassPS = RACoN.RTILayer.PSKind.Subscribe;
            // Create Parameters
            SatUserName = new RACoN.RTILayer.CHlaInteractionParameter("SatUserName");
            this.Parameters.Add(SatUserName);

            SatUserAltitude = new RACoN.RTILayer.CHlaInteractionParameter("SatUserAltitude");
            this.Parameters.Add(SatUserAltitude);

            SatUserOrbitalInclination = new RACoN.RTILayer.CHlaInteractionParameter("SatUserOrbitalInclination");
            this.Parameters.Add(SatUserOrbitalInclination);

            SatUserRightAscensionOfAscendingNode = new RACoN.RTILayer.CHlaInteractionParameter("SatUserRightAscensionOfAscendingNode");
            this.Parameters.Add(SatUserRightAscensionOfAscendingNode);

        }

    }
}
