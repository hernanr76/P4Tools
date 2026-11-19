# P4Tools

P4 Annotate opens a local file's Perforce annotation via **File → Open** or when you pass a file path on the command line. **File → Get Info** runs `p4 info`. Successful commands display the output in the app and open a `.txt` copy from the Windows temporary folder in your selected editor.

Use **File → Setup** to select Notepad, find Visual Studio Code, or browse to another editor's `.exe`. In the same window, browse to your `P4.exe` and select **Save**. The app uses your saved paths immediately.

Defaults are defined in `App.config` (deployed as `WpfTool.exe.config`). Setup saves your choices to the per-user `user.config` file.
