using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Interfaces
{
    public class ImportTempInterfaces
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterfacesId { get; set; }

        public string Country { get; set; }


        public string RadiocommunicationService { get; set; }


        public string AllocationNotes { get; set; }

        public string Application { get; set; }


        public string ApplicationNotes { get; set; }


        public string LowerFrequency { get; set; }

        public string UpperFrequency { get; set; }

        public string FrequencyBandNotes { get; set; }

        public string Channeling { get; set; }

        public string ChannelingNotes { get; set; }


        public string Modulation { get; set; }


        public string ModulationNotes { get; set; }


        public string DirectionSeparation { get; set; }

        public string DirectionSeparationNotes { get; set; }
        

        public string TransmitPower { get; set; }


        public string TransmitPowerNotes { get; set; }


        public string ChannelAccessRules { get; set; }


        public string ChannelAccessNotes { get; set; }


        public string AuthorisationRegime { get; set; }


        public string AuthorisationRegimeNotes { get; set; }

        public string AdditionalEssentialRequirements { get; set; }

        public string AdditionalEssentialRequirementsNotes { get; set; }


        public string FrequencyPlanningAssumptions { get; set; }

        public string FrequencyPlanningAssumptionsNotes { get; set; }


        public string PlannedChanges { get; set; }

        public string PlannedChangesNotes { get; set; }


        public string Reference { get; set; }

        public string ReferenceNotes { get; set; }

        public string Notification { get; set; }

        public string NotificationNotes { get; set; }


        public string Remarks { get; set; }


        public string RemarksNotes { get; set; }
    }
}
