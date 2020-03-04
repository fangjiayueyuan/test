'''
保留方法的学习
'''
class DemoSave:
    def __init__(self,name):
        self.name = name
    def __len__(self):
        return len(self.name)

dc1 = DemoSave("yueyang")
dc2 = DemoSave("方金榜")
print(len(dc1))
print(len(dc2))
