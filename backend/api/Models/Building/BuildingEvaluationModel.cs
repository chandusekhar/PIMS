using System;

namespace Pims.Api.Models.Building
{
    public class BuildingEvaluationModel : BaseModel
    {
        #region Properties
        public int BuildingId { get; set; }

        public DateTime Date { get; set; }

        public string Key { get; set; }

        public float Value { get; set; }

        public string Note { get; set; }
        #endregion
    }
}
