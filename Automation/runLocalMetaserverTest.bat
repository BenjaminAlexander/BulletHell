start %1
%2
SET RESULT=set VAR=%errorlevel%
taskkill /im Metaserver.exe
exit %RESULT%