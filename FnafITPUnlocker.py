import os
import customtkinter as ctk

UNLOCK_FILE = os.path.join(os.getcwd(), "unlock.txt")

# The exact backend keys required by the bridge
ACHIEVEMENTS = [
    "INTO_THE_PIT", "ITS_ME", "I_HEAR_YOU",
    "BIG_KID", "TODDLER", "BABY", "LET_THERE_BE_LIGHT",
    "THE_LIGHT_OF_87", "NIGHT_VISION", "A_MASTER_OF_STEALTH",
    "GOOOAL", "JINXS_FAVORITE", "CHICAS_PARTY", "ROCK_AND_ROLL_BONNIE",
    "A_FUN_TIME_WITH_FREDDY", "SUGAR_RUSH", "HELLO?_HELLO?_HELLO?",
    "IMPATIENT", "WHO_YOU_GONNA_CALL?", "SOCIAL_BUTTERFLY",
    "INSERT_COIN_TO_PLAY", "NOT_A_GAMER", "HONK_HONK", "GIMME_FIVE",
    "WHO_ARE_THEY?", "DONT_JINX_YOURSELF", "SEE_YA_DAD", "WHAT_AM_I",
    "PIT-Y_PARTY", "JEFF?", "FAMILY_FUN_NIGHT", "TERRORFORMER",
    "NIGHT_RIDER", "GOOD_JOB_SPORT", "ACHIEVEMENT_NOT_FOUND",
    "HIDE-N-SEEK_WORLD_CHAMP", "ARCADE_APPRENTICE"
]

def fire_unlock(target_key):
    try:
        with open(UNLOCK_FILE, "w", encoding="utf-8") as f:
            f.write(target_key)
        status_label.configure(text=f"Sent: {target_key}", text_color="#00FF00")
    except Exception as e:
        status_label.configure(text=f"Error: {str(e)}", text_color="#FF0000")

app = ctk.CTk()
app.title("FNAF: ITP - Achievement Unlocker")
app.geometry("450x650")
ctk.set_appearance_mode("dark")
ctk.set_default_color_theme("blue")

status_label = ctk.CTkLabel(app, text=f"Target: {UNLOCK_FILE}", text_color="gray", font=("Segoe UI", 12))
status_label.pack(pady=(0, 15))

# Master Unlock Button
unlock_all_btn = ctk.CTkButton(
    app, 
    text="UNLOCK ALL", 
    fg_color="#238B00", 
    hover_color="#1C5C00",
    font=("Segoe UI", 16, "bold"),
    height=45,
    command=lambda: fire_unlock("UNLOCK_ALL")
)
unlock_all_btn.pack(fill="x", padx=20, pady=(0, 15))

# Scrollable list for individual achievements
scroll_frame = ctk.CTkScrollableFrame(app, label_text="Individual Achievements")
scroll_frame.pack(expand=True, fill="both", padx=20, pady=(0, 20))

# Populate Buttons Dynamically
for ach in ACHIEVEMENTS:
    display_name = ach.replace("_", " ").title()
    btn = ctk.CTkButton(
        scroll_frame,
        text=display_name,
        fg_color="#2b2b2b",
        hover_color="#3a3a3a",
        border_width=1,
        border_color="#444444",
        anchor="w",
        command=lambda k=ach: fire_unlock(k)
    )
    btn.pack(fill="x", pady=2, padx=5)

if __name__ == "__main__":
    app.mainloop()
