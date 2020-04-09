from tkinter import *

class Shell:
    clicked = 0
    def On_Btn_Click(self):
        self.clicked = self.clicked + 1
        self.lbl.configure(text="Someone clicked the button: " +  str(self.clicked))

    def OpenShell(self):       
        # Creates new window and initialize all UI control on the screen.     
        window = Tk()
        window.title("SEDY APP")
        window.geometry("800x600")

        self.lbl = Label(window, text="Hello")
        self.lbl.grid(column=0, row=0)

        btn = Button(window, text="Run", command=self.On_Btn_Click) 
        btn.grid(column=0, row=1)        
        btn.configure(width = 6, height=2)

        window.mainloop()