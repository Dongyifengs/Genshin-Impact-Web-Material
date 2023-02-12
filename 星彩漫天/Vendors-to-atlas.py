import os


def delete(path: str):
    for i in os.listdir(path):
        f_path = os.path.join(path, i)
        if os.path.isdir(f_path):
            delete(f_path)
            continue
        os.remove(f_path)


if os.path.exists("texts"):
    delete("texts")
with open("Vendors.parse.js", "r", encoding="utf-8") as fp:
    fc = fp.readlines()
for l in fc:
    if 'e.exports = "' in l and (".PNG" in l or ".png" in l):
        c = l.replace(" ", "").replace("\n", "").replace('e.exports="', "").replace('"', "")
        n = c.split(".png")[0]
        p = os.path.join("texts", n)
        file_path = os.path.join(p, f"{n}.txt")
        with open(file_path, "w", encoding='utf-8') as io:
            io.write(c)
