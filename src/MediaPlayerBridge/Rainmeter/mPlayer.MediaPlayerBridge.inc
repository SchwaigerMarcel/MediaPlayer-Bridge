; MediaPlayerBridge adapter for piXel VISUALIZER / classic NowPlaying skins.
; Installed automatically by: MediaPlayerBridge.exe --install-pixel

[MPB_Data]
Measure=Plugin
Plugin=WebParser
URL=http://127.0.0.1:8974/now-playing
RegExp="(?si)\A\{.*?\"Title\":\"(.*?)\".*?\"Artist\":\"(.*?)\".*?\"Album\":\"(.*?)\".*?\"Genre\":\"(.*?)\".*?\"StateIcon\":\"(.*?)\".*?\"PositionText\":\"(.*?)\".*?\"DurationText\":\"(.*?)\".*?\"PositionSeconds\":(.*?),\"DurationSeconds\":(.*?),\"CoverPath\":\"(.*?)\".*?\}"
UpdateRate=30
DynamicVariables=1

[mTitulo]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=1
DynamicVariables=1

[mArtista]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=2
DynamicVariables=1

[mAlbum]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=3
DynamicVariables=1

[mGenero]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=4
DynamicVariables=1

[mEstado]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=5
DynamicVariables=1

[mPosicion]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=6
DynamicVariables=1

[mDuracion]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=7
DynamicVariables=1

[mPosicionSegundos]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=8
DynamicVariables=1

[mDuracionSegundos]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=9
DynamicVariables=1

[mCaratula]
Measure=Plugin
Plugin=WebParser
URL=[MPB_Data]
StringIndex=10
DynamicVariables=1

[mProgreso]
Measure=Calc
Formula=([mPosicionSegundos] / Max([mDuracionSegundos], 1))
DynamicVariables=1

[mInfo]
Measure=Script
ScriptFile=#@#Measures\Marquee.lua
MeasureName=mTitulo|mArtista
Width=19
Delimiter=" - "
UpdateDivider=4
