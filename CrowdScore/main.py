#bs"d

from kivy.uix.screenmanager import ScreenManager,Screen
from kivy.properties import ObjectProperty
import datetime
import os
import subprocess
from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.lang import Builder
from kivy.core.window import Window
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.settings import SettingsWithTabbedPanel
from kivy.logger import Logger
from kivy.lang import Builder
import pandas as pd
import json
from os.path import join, exists
from kivy.app import App
from kivy.uix.screenmanager import ScreenManager, Screen, SlideTransition
from kivy.properties import ListProperty, StringProperty, \
        NumericProperty, BooleanProperty
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.floatlayout import FloatLayout
from kivy.clock import Clock

Builder.load_file("project.kv")
Builder.load_file("New.kv")
Window.clearcolor = (0,0,0,0)


simulator='simulator'
save_project=''
current_project=''
class MutableTextInput(FloatLayout):

    text = StringProperty()
    multiline = BooleanProperty(True)

    def __init__(self, **kwargs):
        super(MutableTextInput, self).__init__(**kwargs)
        Clock.schedule_once(self.prepare, 0)

    def prepare(self, *args):
        self.w_textinput = self.ids.w_textinput.__self__
        self.w_label = self.ids.w_label.__self__
        self.view()

    def on_touch_down(self, touch):
        if self.collide_point(*touch.pos) and touch.is_double_tap:
            self.edit()
        return super(MutableTextInput, self).on_touch_down(touch)

    def edit(self):
        self.clear_widgets()
        self.add_widget(self.w_textinput)
        self.w_textinput.focus = True

    def view(self):
        self.clear_widgets()
        if not self.text:
            self.w_label.text = "Double tap/click to edit"
        self.add_widget(self.w_label)

    def check_focus_and_view(self, textinput):
        if not textinput.focus:
            self.text = textinput.text
            self.view()


class ProjectView(Screen):

    project_index = NumericProperty()
    project_title = StringProperty()
    project_content = StringProperty()


class ProjectListItem(BoxLayout):

    def __init__(self, **kwargs):
        print(kwargs)
        del kwargs['index']
        super(ProjectListItem, self).__init__(**kwargs)
    project_content = StringProperty()
    project_title = StringProperty()
    project_index = NumericProperty()

class Menu(BoxLayout):
    manager = ObjectProperty(None)

class New(Screen):
    def get_folder_name(self):

        return str(datetime.datetime.now().__hash__())
    def get_project_save(self):

        return save_project

    def save(self, path,folder_name,params):
        global current_project
        current_project=path+'/'+folder_name

        os.mkdir(current_project)
        params = [x.strip() for x in params]
        mxml=open('myXml.xml').read().split("*")

        tmpXml = ""
        for i in range(len(mxml) - 1):
            tmpXml = tmpXml + mxml[i] + str(params[i])

        tmpXml = tmpXml + mxml[-1]
        tmpXml = str(tmpXml)
        f = open(simulator+"/SimExe.xml", 'w')
        f.write(tmpXml)
        f.close()


        return

    def run(self):

        return str(os.system(simulator+'/TaskSimulationCmd.exe'))
    def visual(self):
        import grp
        grp.GraphicalResults()
        return

    pass


class Projects(Screen):

    data = ListProperty()

    def args_converter(self, row_index, item):
        return {
            'project_index': row_index,
            'project_content': item['content'],
            'project_title': item['title']}



class Manager(ScreenManager):
    newScreen = ObjectProperty(None)
    projectsScreen=ObjectProperty(None)



class CrowdScoreApp(App):

    def build(self):
        global simulator

        simulator = self.config.get('CrowdScore', 'simulator_path')
        global save_project
        save_project= self.config.get('CrowdScore', 'project_path')

        self.default_simulation = " ,  ,   ,AQL, 0 ,true, Exponential, 1,Exponential ,0.208,1 2 4"
        self.simulation_content = self.default_simulation
        root=Menu()

        self.projects = root.manager.projectsScreen
        self.load_projects()
        self.transition = SlideTransition(duration=.35)
        root.manager.transition=self.transition

        return root
    def open_simulation(self, content):
        self.simulation_content=content
        print(self.simulation_content)


    def new_simulation(self):
        self.simulation_content = self.default_simulation



    def load_projects(self):
        if not exists(self.projects_fn):
            return
        with open(self.projects_fn) as fd:
            data = json.load(fd)

        self.projects.data = data

    def save_projects(self):
        with open(self.projects_fn, 'w') as fd:
            json.dump(self.projects.data, fd)

    def del_project(self, project_index):
        del self.projects.data[project_index]
        self.save_projects()
        self.refresh_projects()
        self.go_projects()

    def edit_project(self, project_index):

        project = self.projects.data[project_index]
        name = 'project{}'.format(project_index)

        if self.root.manager.has_screen(name):
            self.root.manager.remove_widget(self.root.manager.get_screen(name))

        view = ProjectView(
            name=name,
            project_index=project_index,
            project_title=project.get('title'),
            project_content=project.get('content'))

        self.root.manager.add_widget(view)
        self.transition.direction = 'left'
        self.root.manager.current = view.name

    def add_project(self,content):

        self.projects.data.append({'title': current_project, 'content': content})
        print(content)
        project_index = len(self.projects.data) - 1
        self.save_projects()
        self.refresh_projects()
        #self.edit_project(project_index)

    def set_project_content(self, project_index, project_content):

        self.projects.data[project_index]['content'] = project_content
        data = self.projects.data
        self.projects.data = []
        self.projects.data = data
        self.save_projects()
        self.refresh_projects()

    def set_project_title(self, project_index, project_title):
        self.projects.data[project_index]['title'] = project_title
        self.save_projects()
        self.refresh_projects()

    def refresh_projects(self):
        data = self.projects.data
        self.projects.data = []
        self.projects.data = data

    def go_projects(self):
        self.transition.direction = 'right'
        self.root.manager.current = 'projects'

    @property
    def projects_fn(self):
        return join(self.user_data_dir, 'projects.json')

    def build_config(self, config):

            config.setdefaults('CrowdScore', {'simulator_path': 'simulator', 'project_path': ''})

    def build_settings(self, settings):

            settings.add_json_panel('CrowdScore', self.config, 'setting.json')


    def on_config_change(self, config, section, key, value):
            if section == "CrowdScore":
                if key == "simulator_path":
                    global simulator
                    simulator= value
                elif key == 'project_path':
                    global save_project
                    save_project= value


    def close_settings(self, settings=None):

            super(CrowdScoreApp, self).close_settings(settings)




if __name__ == '__main__':
    CrowdScoreApp().run()