from json import loads
import os


def delete(path: str):
    for i in os.listdir(path):
        f_path = os.path.join(path, i)
        if os.path.isdir(f_path):
            delete(f_path)
            continue
        os.remove(f_path)


if os.path.exists("texts1"):
    delete("texts1")
os.mkdir("texts1")

with open("index.parse.js", "r", encoding='utf-8') as fp:
    lines = fp.read().split("\n")
for l in lines:
    if "e.exports = JSON.parse('" in l:
        json_content = l.replace("e.exports = JSON.parse('", "").replace("')", "")
        dict_json = loads(json_content)
        h = dict_json['skeleton']["hash"]
        dir_path = os.path.join("texts1", h)
        os.mkdir(dir_path)
        with open(os.path.join(dir_path, f"{h}.txt"), 'w', encoding='utf-8') as fp:
            fp.write(json_content.replace(" ",""))