start %1
%2
SET RESULT=%errorlevel%
taskkill /im Metaserver.exe
exit %RESULT%