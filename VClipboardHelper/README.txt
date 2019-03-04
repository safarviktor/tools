========= Clipboard helper =======
Text transformation tool. Current version provides these transformations on text in clipboard:

- ugly SQL → semi-pretty SQL
-- it's not very smart, so far adding a new line before some key words and capitalizing most of SQL keywords
-- handy when you have a large query as one-liner
-- could use a SQL parser and do it properly but no time!

- list → CSV
- CSV → single quoted CSV 
- Stack Trace formatting
- SQL parameters to SQL declaration + value set


======== How to =======
- Get the exe file
- Place a shortcut on you desktop
- Go to the shortcut properties
- Set a Shortcut Key
-- eg. I use CTRL+ALT+C which means that I can do a sequence CTRL+C - CTRL+ALT+C - CTRL+ALT+C to transform a column from DB into a SQL-friendly quoted CSV
