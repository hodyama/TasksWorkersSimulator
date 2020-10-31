#bsd
import shutil
import matplotlib
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import fnmatch
import os
import xlsxwriter
from distutils.dir_util import copy_tree
from tkinter.filedialog import*

matplotlib.use("WebAgg")



fig_c = 0
def GraphicalResults():

    def plotScoringGraph(path, worksheet, location):
        global fig_c
        df = pd.read_csv(path).replace({np.nan: 0})

        df = df.set_index(df.columns[0])
        df = df[df.columns[:-1]]
        if len(df) > 1000:
            df = df[:-1000]

        df.plot(figsize=[10, 5]).get_figure().savefig("fig\g" + str(fig_c) + ".png")
        worksheet.insert_image(location, "g" + str(fig_c) + ".png")
        fig_c += 1

    def plotGroupLineGragh(path, worksheet, location):
        global fig_c
        df = pd.read_csv(path).replace({np.nan: 0})

        df = df.set_index(df.columns[0])

        if len(df) > 1000:
            df = df[:-1000]

        df.plot(figsize=[10, 5]).get_figure().savefig("fig\g" + str(fig_c) + ".png")
        worksheet.insert_image(location, "g" + str(fig_c) + ".png")
        fig_c += 1

    def plotGroupScatterGraph(path, worksheet, location):
        global fig_c
        df = pd.read_csv(path).replace({np.nan: 0})

        df.plot.scatter(df.columns[0], df.columns[1], xticks=range(1, len(df) + 1),
                        figsize=[10, 5]).get_figure().savefig("fig\g" + str(fig_c) + ".png")
        worksheet.insert_image(location, "g" + str(fig_c) + ".png")
        fig_c += 1

    def plotLineGraph(path, worksheet, location):
        global fig_c
        df = pd.read_csv(path)
        df = df.set_index(df.columns[0])
        loc = int(location[1:])
        for i in range(0, len(df), 2):
            tmp = df[i:i + 2]
            tmp = tmp.T
            tmp = tmp.set_index(tmp.columns[0])
            tmp.plot(figsize=[10, 5]).get_figure().savefig("fig\g" + str(fig_c) + ".png")
            loc = loc + 25
            worksheet.insert_image(location[0] + str(loc), "g" + str(fig_c) + ".png")
            fig_c += 1

    def plotScatterGragh(path, worksheet, location):
        global fig_c
        df = pd.read_csv(path)
        df = df.set_index(df.columns[0])
        loc = int(location[1:])
        for i in range(0, len(df), 2):
            tmp = df[i:i + 2]
            tmp = tmp.T

            tmp.plot.scatter(tmp.columns[0], tmp.columns[1], figsize=[10, 5]).get_figure().savefig(
                "fig\g" + str(fig_c) + ".png")
            loc = loc + 25
            worksheet.insert_image(location[0] + str(loc), "g" + str(fig_c) + ".png")
            fig_c += 1

    dir = fnmatch.filter(os.listdir(), 'Simulation*')[0]



    workbook = xlsxwriter.Workbook(dir + '/graphs.xlsx')

    worksheet = workbook.add_worksheet("scoring function")
    plotScoringGraph(dir + "/WorkersGradesAtArrivalTask.csv", worksheet, 'A1')

    os.remove(dir + "/WorkersGradesAtArrivalTask.csv")

    worksheet = workbook.add_worksheet("queue length")
    plotGroupScatterGraph(dir + "/AvgQueueLength.csv", worksheet, 'A1')

    os.remove(dir + "/AvgQueueLength.csv")

    plotLineGraph(dir + "/WorkersQueueAtTime.csv", worksheet, 'A11')
    os.remove(dir + "/WorkersQueueAtTime.csv")

    worksheet = workbook.add_worksheet("utilization")
    plotGroupScatterGraph(dir + "/WorkersUtilization.csv", worksheet, 'A1')

    os.remove(dir + "/WorkersUtilization.csv")

    plotLineGraph(dir + "/WorkersBusyAtTime.csv", worksheet, 'A11')
    os.remove(dir + "/WorkersBusyAtTime.csv")

    worksheet = workbook.add_worksheet("processing time")
    plotGroupScatterGraph(dir + "/AvgTasksProcessingTime.csv", worksheet, 'A1')

    os.remove(dir + "/AvgTasksProcessingTime.csv")

    plotScatterGragh(dir + "/TasksProcessingTime.csv", worksheet, 'A11')
    os.remove(dir + "/TasksProcessingTime.csv")
    worksheet = workbook.add_worksheet("waiting time")
    plotGroupScatterGraph(dir + "/AvgTasksWaitingTimePerWorker.csv", worksheet, 'A1')

    os.remove(dir + "/AvgTasksWaitingTimePerWorker.csv")

    plotScatterGragh(dir + "/TasksWaitingTime.csv", worksheet, 'A11')
    os.remove(dir + "/TasksWaitingTime.csv")
    worksheet = workbook.add_worksheet("allocated and finished task")
    plotGroupLineGragh(dir + "/SumOfWorkerFinishedTsks.csv", worksheet, 'A1')
    os.remove(dir + "/SumOfWorkerFinishedTsks.csv")
    workbook.close()

    #for img in fnmatch.filter(os.listdir(), 'g*.png'):
        #os.remove(img)

    shutil.copyfile("Simulator/simExe.xml", dir + "/configuration.xml")
    dst_dir = askdirectory() + "/" + dir
    os.mkdir(dst_dir)

    copy_tree(dir, dst_dir)
    shutil.rmtree(dir)









