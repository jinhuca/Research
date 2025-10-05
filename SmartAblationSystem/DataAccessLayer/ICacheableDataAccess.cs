using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public interface ICacheableDataAccess
    {
        CatheterType GetCatheterByCatheterId(int catheterId);
        IEnumerable<PMCRegisterValue> GetPMCRegisterValuesByCatheterID(int catheterID);
        IEnumerable<CMCRegisterValue> GetCMCRegisterValuesByCatheterID(int catheterID);
        BalloonParameters GetDASBalloonParameterByStateId(int stateId);
        double GetCurrentTankMetalWeight();

        Tuple<long, string, string, string> GetErrorAndSolutionTranslationsForCurrentLanguage(int errorId, int errorType);
        Tuple<long, string, string, string, int> GetErrorMessageWithErrorTypeById(int errorId, int errorType);
    }
}