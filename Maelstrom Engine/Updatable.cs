using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom_Engine {
    public interface Updatable {
        void Update(float deltaTime);
    }
}
