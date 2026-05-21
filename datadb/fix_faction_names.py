import os

folders = [
    r'C:\Users\Texy6\Desktop\wrobot profile\HB_Converted',
    r'C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Default Profiles\GB\wrobot_converted',
]

# Files with wrong [A-H] prefix -> correct faction
renames = {
    '[A-H][Mining - 1-65] Durotar_HB.xml':            '[Horde][Mining - 1-65] Durotar_HB.xml',
    '[A-H][Mining - 1-65] Durotar - Ground_HB.xml':   '[Horde][Mining - 1-65] Durotar - Ground_HB.xml',
    '[A-H][Herbs - 1-75] Mulgore_HB.xml':             '[Horde][Herbs - 1-75] Mulgore_HB.xml',
    '[A-H][Mining-Herbs - 1-75] ElwynForest_HB.xml':  '[Alliance][Mining-Herbs - 1-75] ElwynForest_HB.xml',
}

for folder in folders:
    for old, new in renames.items():
        src = os.path.join(folder, old)
        dst = os.path.join(folder, new)
        if os.path.exists(src):
            if os.path.exists(dst):
                os.remove(dst)
            os.rename(src, dst)
            print(f'  {old} -> {new}')
        elif os.path.exists(dst):
            print(f'  (already correct) {new}')
        else:
            print(f'  NOT FOUND: {old}')
    print(f'Done: {folder}')
