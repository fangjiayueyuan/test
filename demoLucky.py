'''
用来计算某人的幸运数字
'''
class DemoLucky:
    def __init__(self,name):
        self.name = name
    def Lucky(self):
        s = 0
        for c in self.name:
            s+=ord(c)%100
        return s
dc1 = DemoLucky("金榜")
dc2 = DemoLucky("岳阳")

print(dc1.Lucky())
print(dc2.Lucky())