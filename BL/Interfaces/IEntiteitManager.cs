using Domain.Entiteit;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IEntiteitManager
    {
        void CreateTestData();
        List<Domain.Entiteit.Entiteit> getAlleEntiteiten();
        void updateEntiteit(Entiteit entiteit);
        bool berekenTrends(double minVoorwaarde, Entiteit entiteit, TrendType type, Domain.Enum.Voorwaarde voorwaarde);
        void ResetTrends();
    }
}
