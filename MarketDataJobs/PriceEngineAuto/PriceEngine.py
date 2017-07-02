
from MarketData import MarketData
import pandas




class Robot:
    counter = 0
    def __init__(self, name):
        self.name = name
    def sayHello(self):
        return "Hi, I am " + self.name
def Rob_init(self, name):
    self.name = name
RobotX = type("Robot2", 
              (), 
              {"counter":0, 
               "__init__": Rob_init,
               "sayHello": lambda self: "Hi, I am " + self.name})
x = RobotX("Marvin")
print(x.name)
print(x.sayHello())
y = Robot("Marvin")
print(y.name)
print(y.sayHello())
print(x.__dict__)
x.__dict__['hero'] = 32
print(x.hero)
print(y.__dict__)
print(type(RobotX))

#print(result.dtypes)


a = MarketData()

a.open()
result = a.read("select * from [MarketData].[sec].[InstrumentID]")