using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public partial class DataAccess 
    {
        private static List<ErrorMessage> _errorMessagesCache = new List<ErrorMessage>();

        private IDictionary<int, List<PMCRegisterValue>> _pmcRegisterValuesByCatheterIdDict = new Dictionary<int, List<PMCRegisterValue>>();
        private IDictionary<int, List<CMCRegisterValue>> _cmcRegisterValuesByCatheterIdDict = new Dictionary<int, List<CMCRegisterValue>>();
        private IDictionary<int, BalloonParameters> _balloonParameters = new Dictionary<int, BalloonParameters>(); 
        private IDictionary<int, CatheterType> _catheterTypeByIdDict = new Dictionary<int, CatheterType>();

        public CatheterType GetCatheterByCatheterId(int catheterId)
        {
            if (_catheterTypeByIdDict.ContainsKey(catheterId))
            {
                return _catheterTypeByIdDict[catheterId];
            }

            var catheterType = GetCatheterAccordingToCatheterId(catheterId);
            if (catheterType != null) _catheterTypeByIdDict[catheterId] = catheterType;

            return catheterType; 
        }

        public IEnumerable<PMCRegisterValue> GetPMCRegisterValuesByCatheterID(int catheterID)
        {
            if (_pmcRegisterValuesByCatheterIdDict.ContainsKey(catheterID))
            {
                return _pmcRegisterValuesByCatheterIdDict[catheterID]; 
            }

            var pmcRegisterValues = GetPMCRegisterValuesAccordingToCatheterID(catheterID);
            if (pmcRegisterValues != null && pmcRegisterValues.Any()) 
                _pmcRegisterValuesByCatheterIdDict[catheterID] = pmcRegisterValues;

            return pmcRegisterValues;
        }

        public IEnumerable<CMCRegisterValue> GetCMCRegisterValuesByCatheterID(int catheterID)
        {
            if (_cmcRegisterValuesByCatheterIdDict.ContainsKey(catheterID))
            {
                return _cmcRegisterValuesByCatheterIdDict[catheterID]; 
            }

            var cmcRegisterValues = GetCMCRegisterValuesAccordingToCatheterID(catheterID);
            if (cmcRegisterValues != null && cmcRegisterValues.Any())
                _cmcRegisterValuesByCatheterIdDict[catheterID] = cmcRegisterValues;

            return cmcRegisterValues;
        }

        public BalloonParameters GetDASBalloonParameterByStateId(int stateId)
        {
            if (_balloonParameters.ContainsKey(stateId))
            {
                return _balloonParameters[stateId];
            }

            var dasBalloonParameter = GetDASBalloonParameterAccordingToStateId(stateId); 
            if (dasBalloonParameter != null) _balloonParameters[stateId] = dasBalloonParameter;

            return dasBalloonParameter;
        }

        public double GetCurrentTankMetalWeight()
        {
          using (var context = new DataAccessContainer())
          {
            var currentTankType = from console in context.Consoles
              join tank in context.Tanks on console.CurrentTank equals tank.Id
              join tankType in context.TankTypes on tank.Type equals tankType.Id
              select tankType;

            return currentTankType?.FirstOrDefault()?.MetalWeight ?? 0d;
          }
        }

        private void InitializeCache()
        {
            //DAS BallonParameters
            _balloonParameters = GetDASBallonParameters()
                .GroupBy(v => v.StateID)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            // CatheterType by CatheterId
            _catheterTypeByIdDict = GetCatheterTypes()
                .GroupBy(c => c.CatheterID)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            _pmcRegisterValuesByCatheterIdDict = GetPMCRegisterValues()
                .GroupBy(v => v.CatheterTypeID)
                .ToDictionary(g => g.Key, g => g.ToList());

            _cmcRegisterValuesByCatheterIdDict = GetCMCRegisterValues()
                .GroupBy(v => v.CatheterTypeID)
                .ToDictionary(g => g.Key, g => g.ToList());

            lock (_errorMessagesCache)
            {
              if (!_errorMessagesCache.Any())
              {
                _errorMessagesCache.AddRange(GetAllErrorMessages());
              }
            }
        }

        private IEnumerable<ErrorMessage> GetAllErrorMessages()
        {
          int currentLanguage = GetCurrentLanguageId();

          using (var context = new DataAccessContainer())
          {
            var errors = (from p in context.ErrorMessages
              where p.LanguageId == currentLanguage
              select p).ToList();

            return errors;
          }
        }

      /// <summary>
      /// Function that gets all CatheterTypes from Catheter table in the database
      ///. Safety classification: Death or SERIOUS INJURY is possible (IEC 62304 Class C).
      /// </summary>
      /// <id>SF-SDS-0008</id>
      /// <returns>All the catheter types in the database.</returns>
      public IEnumerable<CatheterType> GetCatheterTypes()
        {
            using (var context = new DataAccessContainer())
            {
                IQueryable<CatheterType> catheter = from p in context.CatheterTypes
                    select p;
                return catheter.ToList();
            }
        }

        private IEnumerable<PMCRegisterValue> GetPMCRegisterValues()
        {
            using (var context = new DataAccessContainer())
            {
                IQueryable<PMCRegisterValue> pMCRegisterValues = from p in context.PMCRegisterValues
                    select p;
                return pMCRegisterValues.ToList();
            }
        }

        private BalloonParameters GetDASBalloonParameterAccordingToStateId(int stateId)
        {
            using (var context = new DataAccessContainer())
            {
                IQueryable<BalloonParameters> ballonParameters = from p in context.BalloonParameters
                                                                 where p.StateID == stateId
                                                                 select p;
                return ballonParameters.FirstOrDefault();
            }
        }

  }
}
