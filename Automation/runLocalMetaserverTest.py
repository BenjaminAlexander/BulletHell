import subprocess
import sys

#start %1
#%2
#SET RESULT=%errorlevel%
#taskkill /im Metaserver.exe
#exit %RESULT%

metaserver = subprocess.Popen(["Artifacts\Metaserver.exe"],
    stdout=sys.stdout,
    stdin=sys.stdin,
    stderr=sys.stderr);

metaserverTest = subprocess.Popen(["Artifacts\MetaserverTest.exe"],
    stdout=sys.stdout,
    stdin=sys.stdin,
    stderr=sys.stderr);

metaserverTest.wait();
metaserver.terminate();

if(metaserverTest.returncode != 0):
    sys.exit("MetaserverTest failed");
sys.exit(0)
