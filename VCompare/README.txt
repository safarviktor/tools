A tool to compare random files using the built-in Visual Studio diff tool.

It supports

- drag & drop of 1 by 1 file
- drag & drop both files at the same time (GUI hopefully intutitive)
- paste the full file path into the respective text box
- paste raw text from text box (will create a temp file)

RED indicates the file has not been found (default on app startup).

Button at the bottom (Launch comparer) launches Visual Studio'd diff tool with the files as parameters. If the button does not work, check that both file locations and the VS IDE path are green.
