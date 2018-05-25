using DAL;

namespace BL.Managers
{
    public class UnitOfWorkManager
    {
        private UnitOfWork _uof;

        internal UnitOfWork UnitOfWork
        {
            get
            {   //Om via buitenaf te verzekeren dat er géén onnodige nieuwe
                //instanaties van UnitOfWork geïnstantieerd worden...
                if (_uof == null) _uof = new UnitOfWork();
                return _uof;
            }
        }

        public void Save()
        {
            UnitOfWork.CommitChanges();
        }
    }
}
