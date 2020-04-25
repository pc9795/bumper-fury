* Used text overflow while showing player score and scoreboard.
* Building for Android versions greater than pie(v28) because of a build error in unity for lower versions.
* Awake life cycle is only used by singletons.
* No classes without any variable. No marker classes. If want to mark a specific object use tags.
* Use `none` as value for button's `navigation`.
* Keep everything in `float` your life will be easy.
* Use prefabs
* To implement difficulty following things are done:
    1. Health buff in `GameManager.LoadPlayer` and `GameManager.LoadAIOpponents`.
    2. Energy buff in `GameManger.LoadPlayer` and `GameManager.LoadAIOpponents`.
    3. Intelligent AI in `AIManager.Refersh`. Changed sensor length accordingly and player attacking probability
    4. Affected level time in `LeveTimer.Start`.