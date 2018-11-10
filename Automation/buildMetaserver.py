import subprocess
import sys
import os
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
    p = subprocess.Popen(["dotnet",
                       "build",
                       csProjPath(name),
                       "-c",
                       "Release"],
                      stdout=sys.stdout,
                      stdin=sys.stdin,
                      stderr=sys.stderr);
    p.wait();
    if(p.returncode != 0):
        sys.exit(name + " build failed");

def collectExecutableArtifacts(name):
    copy(exePath(name), os.path.join("Artifacts"))
    copy(exeConfigPath(name), os.path.join("Artifacts"))

if __name__ == "__main__":
    buildCsProj("Metaserver");
    collectExecutableArtifacts("Metaserver");
    buildCsProj("MetaserverTest");
    collectExecutableArtifacts("MetaserverTest");
    sys.exit(0);
