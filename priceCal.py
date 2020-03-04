class Product():  #定义类，保存名字和价格等信息
    def __init__(self,name):
        self.name = name
        self.label_price = 0  # 初始值为0
        self.real_price = 0

c = Product("电脑")
d = Product("打印机")
e = Product("投影仪")

c.label_price,c.real_price = 10000,8000
d.label_price,d.real_price = 2000,1000
e.label_price,e.real_price = 1500,900

s1,s2 = 0,0

for i in [c,d,e]:
    s1+=i.label_price
    s2+=i.real_price

print(s1,s2)

