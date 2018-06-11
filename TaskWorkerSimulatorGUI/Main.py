#bs"d
from tkinter.filedialog import asksaveasfilename
from tkinter.filedialog import askopenfilename
import xml

import matplotlib
matplotlib.use("TkAgg")
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg, NavigationToolbar2TkAgg
from matplotlib.figure import Figure
import matplotlib.animation as animation
from matplotlib import style
from tkinter import *
import tkinter as tk
from tkinter import ttk
import os



BACKGROUND='#333333'
LARGE_FONT= ("Verdana", 12,'bold')
MEDIUM_FONT=("Arial",12)
style.use("ggplot")

f = Figure(figsize=(5 ,5), dpi=100)
a = f.add_subplot(111)


class TaskWorkerSimulatorGUI(tk.Tk):

    def __init__(self, *args, **kwargs):
        tk.Tk.__init__(self, *args, **kwargs)
        tk.Tk.wm_title(self, "TaskWorkerSimulator")
        tk.Tk.geometry(self,'1000x500')


        container = tk.Frame(self,bg="black")


        container.pack(side="top", fill="both", expand=True)


        container.grid_rowconfigure(0, weight=1)
        container.grid_columnconfigure(0, weight=1)

        self.frames = {}

        for F in (StartPage, Grahps_Page):

            frame = F(container, self)

            self.frames[F] = frame

            frame.grid(row=0, column=0, sticky="nsew")



        self.show_frame(StartPage)




    def show_frame(self, cont):
        frame = self.frames[cont]
        frame.tkraise()





class StartPage(tk.Frame):

    def ask_file(self,rf):
        file_name = askopenfilename(filetypes=(("XML file", "*.xml"), ("All Files", "*.*")),
                              defaultextension='.xml', title="select configuration file")
        mxml=open(file_name).readlines()
        f=open("Release/simExe.xml",'w')
        f.writelines((mxml))
        f.close()
        file=ttk.Label(rf, text=file_name, background=BACKGROUND, foreground='white')
        file.pack(pady=10, padx=10)


    def simulate(self,bf, cont):

            os.system("Release\TaskSimulationCmd.exe")
            import grp
            grp.GraphicalResults()
            #cont.show_frame(Grahps_Page)


    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent,bg=BACKGROUND)

        tf=tk.Frame(self, bg=BACKGROUND,relief='sunken', bd=4)
        tf.pack(side="top", fill="both")
        flabel = ttk.Label(tf, text="SIMULATOR FOR ALLOCATING EMPLOYEES IN THE CROWDESOURCING MARKET",
                           background=BACKGROUND, foreground='red', font=LARGE_FONT)

        flabel.pack( pady=10, padx=10)

        slabel = ttk.Label(tf, text="set configuration to start simulation",background=BACKGROUND, foreground='red', font=MEDIUM_FONT)
        slabel.pack(pady=10, padx=10)

        bf = tk.Frame(self, bg=BACKGROUND, relief='sunken', bd=4)
        bf.pack(side='bottom', fill="both", expand=True)
        simbt = ttk.Button(bf, text="Simulate", command=lambda: self.simulate(bf,controller))
        simbt.pack(pady=20, padx=20)

        rf=tk.Frame(self,bg=BACKGROUND,relief='sunken', bd=4)
        rf.pack(side='right', fill="both", expand=True)
        upLabel=ttk.Label(rf, text="upload setting file",background=BACKGROUND, foreground='red', font=MEDIUM_FONT)
        upLabel.pack()
        loadFile= ttk.Button(rf, text="Browse", command=lambda:self.ask_file(rf))
        loadFile.pack( pady=20, padx=20)
        lf=tk.Frame(self, bg=BACKGROUND,relief='sunken', bd=4)
        lf.pack(side='right', fill="both", expand=True)




class Grahps_Page(tk.Frame):

    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent)
        label = tk.Label(self, text="Graph Page!", font=LARGE_FONT)
        label.pack(pady=10, padx=10)

        menubar = tk.Menu(self)
        filemenu = tk.Menu(menubar, tearoff=0)

        menubar.add_cascade(label="File", menu=filemenu, command=lambda: print("yyyyyyyyyyyyyyyyyy"))
        tk.Tk.config(self)



app = TaskWorkerSimulatorGUI()
app.mainloop()