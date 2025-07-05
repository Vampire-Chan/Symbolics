# 📂 Folder Linker GUI

A simple Windows application that helps you **move folders** (like Google AppData or Program Files directories) to a different drive and create **symbolic links** back in their original place. Perfect for freeing up space on your system drive while keeping applications running smoothly.

---

## 🧰 Features

- ✅ Move multiple folders at once
- 🔗 Automatically create symbolic links
- 🔒 Admin permission check and relaunch
- 📁 Visual folder picker UI (WinForms)
- 💬 Real-time log of actions
- 🧠 Smart skip if already moved or linked

---

## 📸 Screenshots

> Coming soon...

*You can submit screenshots via Pull Requests if you'd like to help!*

---

## 📥 Download

> # 💾 Download through Releases

- Unzip the file
- Run `FolderLinkerGUI.exe` as Administrator
- Start linking your folders!

---

## 🚀 How to Use

1. Click **"Add Source Folder"** to select one or more folders (e.g., `C:\Program Files\Google\Chrome`)
2. Click **"Select Target Folder"** to pick a new location (e.g., `D:\Applications\Google`)
3. Click **"Execute"**
4. Done! Files are moved, and symbolic links are created.

> Tip: Run the app as **Administrator** or it will auto-relaunch with elevated permissions.

---

## 💬 FAQ

### ❓ Does this work across drives?
✅ Yes. It supports moving from `C:\` to `D:\`, `E:\`, etc., and links work as expected.

### ❓ What if the folder already exists at destination?
✔️ It will skip copying and just create the symlink if not already present.

### ❓ Can I rollback the changes?
🛠 A rollback feature is coming soon — you can manually move folders back and delete the symbolic links.

### ❓ Is it safe for Google Play Games AppData?
📂 Yes — it works for folders like `C:\Users\YourName\AppData\Local\Google\Play Games`, as long as apps aren't running during the move.

---

## 🧑‍💻 Tech Stack

- Language: C#
- UI: Windows Forms (.NET 6 or .NET Framework 4.8)
- API: `CreateSymbolicLink` from `kernel32.dll`

---

## 📜 License

MIT License © 2025 

---

## 🤝 Contribute

Pull requests, issues, and feature requests are welcome!  
Star ⭐ the repo to support development.

---
