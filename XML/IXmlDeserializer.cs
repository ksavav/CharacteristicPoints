using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacteristicPoints.XML
{
    public interface IXmlDeserializer
    {
        public List<List<string>> Load(string path);
    }
}
