# MediaPlayer Bridge

A small Windows background bridge for Rainmeter media skins. It reads Windows' native media sessions, so it supports Spotify Desktop, Spotify Web, YouTube Music and other players that expose Windows media controls.

It is designed for the **piXel VISUALIZER by HiTBiT-PA** Rainmeter skin and fixes its missing Spotify metadata, cover art and playback controls.

## What it does

- Shows title, artist, album, genre, progress and cover art in piXel.
- Controls play/pause, next, previous and stop from the skin.
- Works with supported desktop apps and browser tabs, not only Spotify.
- Uses Windows media events instead of audio capture or constant polling.
- Starts automatically with Windows when enabled.

The visualizer bars remain Rainmeter's normal system-audio visualizer, therefore they react to all audible applications.

## Install

### Before you start

- Windows 10 version 1903 or newer
- Rainmeter
- The original **piXel VISUALIZER** RMSKIN, already installed

### Setup

1. Download the current `MediaPlayerBridge-win-x64.zip` from **Actions → Build Windows release → Artifacts**. A tagged version can also appear under **Releases**.
2. Right-click the ZIP → **Properties** → select **Unblock**, then extract it to a permanent folder, for example `C:\Apps\MediaPlayerBridge`.
3. Double-click **Install MediaPlayerBridge.cmd**.
4. Select **1 – Alles einrichten**.
5. Refresh the piXel skin in Rainmeter once.

The setup runs without administrator rights. It creates only a per-user Windows Startup entry and a backup of piXel's original NowPlaying configuration.

## Daily use

After setup, the bridge starts with Windows. Open Spotify, Spotify Web, YouTube Music or another supported player and start playback.

For a browser player, Windows must show normal media controls for that browser tab. If Windows itself has no media session, the bridge cannot retrieve its title or cover art.

## Setup menu

Double-click **Install MediaPlayerBridge.cmd** at any time.

| Option | Result |
| --- | --- |
| 1 | Install or repair piXel support, enable autostart and start the bridge |
| 2 | Install or repair only piXel support |
| 3 | Enable autostart |
| 4 | Start the bridge now |
| 5 | Disable autostart |
| 6 | Uninstall piXel support, autostart and local bridge data |

## Disable autostart

Run **Install MediaPlayerBridge.cmd** and select **5**. The currently running bridge keeps running until you close it or sign out; it will not start after the next login.

## Uninstall

1. Double-click **Install MediaPlayerBridge.cmd**.
2. Select **6 – Deinstallieren**.
3. Refresh piXel in Rainmeter.
4. Delete the extracted `MediaPlayerBridge` program folder.

Uninstall restores piXel's original NowPlaying measures, restores its original playback actions, removes the user Startup entry and deletes the bridge's local cover-art cache.

## Security note

This is a personal, open-source tool. Windows may show a SmartScreen warning for a new unsigned download. Inspect the source and download only from this repository; then use the ZIP's **Unblock / Zulassen** property before extraction. The bridge does not require administrator access and does not disable Windows security features.

## Development and builds

GitHub Actions creates a self-contained Windows ZIP on every push to `main`. Push a tag such as `v1.0.0` to create a GitHub Release automatically.
