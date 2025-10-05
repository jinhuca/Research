using System.Collections.Generic;

namespace DataAccessLayer
{
    public interface IDataAccess
    {
        List<Patient> GetAllPatient();

        List<Physician> GetAllPhysicians();
    }
}