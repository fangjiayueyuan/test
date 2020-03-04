'''
自由方法的学习
'''
class demoStatic:
    count = 0
    def __init__(self,name):
        self.name = name
        demoStatic.count+=1
    def foo():
        demoStatic.count *= 100
        return demoStatic.count

dc1 = demoStatic("岳阳")
dc2 = demoStatic("金榜")
# print(dc1.foo())
print(demoStatic.foo())
# print(dc2.foo())
