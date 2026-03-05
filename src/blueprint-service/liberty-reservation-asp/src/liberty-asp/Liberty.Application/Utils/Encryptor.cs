using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public class Encryptor
    {
        public static Encryptor Instance { get; set; }

        private Encryptor()
        {

        }

        public static Encryptor GetInstance() 
        {
            if(Encryptor.Instance == null)
            {
                Encryptor.Instance = new Encryptor();
            }
            return Encryptor.Instance;
        }

        public string Create() 
        {
            // FIXME not implement
            return "abc123";
        }

    }
}
