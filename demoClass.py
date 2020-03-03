'''
classmethod使用
'''
class demoClass:
    count = 0
    def __init__(self,name):
        self.name = name
        demoClass.count+=1
    @classmethod
    def getChrCount(cls):
        s = "我爱我的祖国，我爱我的家乡"
        return s[demoClass.count]

dc1 = demoClass("岳阳")
dc2 = demoClass("金榜")
print(demoClass.getChrCount())
print(dc1.getChrCount())