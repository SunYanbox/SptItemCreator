import tkinter as tk
from models.object_context import ObjectContext

oc = ObjectContext()

app = tk.Tk()
app.title('Property Analysis')

stats_mgr_frame = tk.Frame(app)
stats_mgr_frame.pack(side='top')
tk.Label(stats_mgr_frame, text=f'Stats Mgr 数据条数: {len(oc.stats_mgr.data)}').pack(side='top')












if __name__ == '__main__':

    app.mainloop()
