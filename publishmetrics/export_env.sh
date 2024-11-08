#!/bin/bash

echo '#!/bin/bash' > /App/set_env.sh
printenv | sed '/^affinity:container/ d' | sed -r 's/^([a-zA-Z_]+[a-zA-Z0-9_-]*)=(.*)$/export \1="\2"/g' >> /App/set_env.sh
chmod +x /App/set_env.sh