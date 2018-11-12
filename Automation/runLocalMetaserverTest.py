import automationUtils
import sys

def main(args):
    metaserver = automationUtils.popenWithStdFlushing(["Artifacts\Metaserver.exe"], "Metaserver");

    metaserverTest = automationUtils.popenWithStdFlushing(["Artifacts\MetaserverTest.exe", "127.0.0.1"], "MetaserverTest");

    metaserverTest.wait();
    metaserver.terminate();

    if(metaserverTest.returncode != 0):
        return "MetaserverTest failed";
    return 0;

if __name__ == "__main__":
    sys.exit(main(sys.argv));
