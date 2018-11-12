import sys
import os
import automationUtils

from shutil import copy

def projectPath(name):
    return os.path.join("..\\Game\\", name);

def csProjPath(name):
    return os.path.join(projectPath(name), name + ".csproj");

def binReleasePath(name):
    return os.path.join(projectPath(name), "bin\\Release");

def exePath(name):
    return os.path.join(binReleasePath(name), name + ".exe");

def exeConfigPath(name):
    return os.path.join(binReleasePath(name), name + ".exe.config");

def buildCsProj(name):
    p = automationUtils.popenWithStdFlushing(["dotnet",
                       "build",
                       csProjPath(name),
                       "-c",
                       "Release"],
                      "DOTNET " + name);
    
    p.wait();
    if(p.returncode != 0):
        return name + " build failed";

def collectExecutableArtifacts(name):
    copy(exePath(name), os.path.join("Artifacts"))
    copy(exeConfigPath(name), os.path.join("Artifacts"))

def main(args):
    buildCsProj("Metaserver");
    collectExecutableArtifacts("Metaserver");
    buildCsProj("MetaserverTest");
    collectExecutableArtifacts("MetaserverTest");
    return 0;

if __name__ == "__main__":
    sys.exit(main(sys.argv));
