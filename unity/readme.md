Built with Unity 2020.1.9f1

Has an auto scrolling text area, when moving cursor over it will stop so you can manually scroll.

Input field detects if the enter key is pressed and will do the same function as the send button.

The listening method for new incoming data (NetListen.cs), places the data into a static variable and the Update function writes it to the text area, this happens once per frame. Since the incoming data has all null characters, the end is trimmed and a new line is added to the end. I'm unsure if this is the best method.
