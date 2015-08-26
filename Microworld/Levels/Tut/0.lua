tutorialSceneName = "MicroWorld.Graphics.GUI.Scene.Tutorial"
dialogSceneName = "MicroWorld.Graphics.GUI.Scene.Dialog"

setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.ComponentSelector.ComponentSelector", false)
setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.CircuitRunningControl", false)
setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.MapsHUD", false)

setAllComponentsUnavalable()
currentTutorial = 0
lightTicks = 0
setAllowedVisibleRectangle(-400, -200, 400, 200)
setCameraScale(4)
setHistoryEnabled(false)
gameStop()
setCanStart(false)
setCanStop(false)
setCanPause(false)
setCanStep(false)

PowerID = 3
PowerJointID = 0
LedID = 7
LedJointLeftID = 4
LedJointRightID = 5
GroundID = 11
GroundJointID = 8

--------------------------------------------------------------------COMPONENTS--------------------------------------------------------------------

--Called when SelectedComponent in ComponentSelector has changed
function OnSelectedComponentChanged(cur)
	if (currentTutorial == 9 and cur == "Wire") then
		closeTutorial()
	else
		clearSelection()
	end
end

--Called when component is selected
function OnComponentSelected(ID)
  
end

--Called when a component is placed
function OnComponentPlaced(ID)
	if (currentTutorial == 10) then
		if (areJointsConnected(PowerJointID, LedJointLeftID)) then
			closeTutorial()
			setComponentRemovability(ID, false)
		else
			removeComponent(ID)
		end
	elseif (currentTutorial == 12) then
		if (areJointsConnected(LedJointRightID, GroundJointID)) then
			closeTutorial()
			setComponentRemovability(ID, false)
		else
			removeComponent(ID)
		end
	else
		removeComponent(ID)
	end
end

--Called when component is removed
function OnComponentRemoved(ID)
  
end

----------------------------------------------------------------------UPDATES----------------------------------------------------------------------

--Called every GAME update
--eg. sandbox, levels, etc
function OnGameUpdate()
	if ((not isHUDSceneOpened(tutorialSceneName)) and (not isHUDSceneOpened(dialogSceneName))) then
		clearSelection()
		clearClickableAreas()
---------------------------------------------------------------------------------------------------------------------------------------------------
		if (currentTutorial == 0) then
			showTutorial(
[[Hello, and welcome to Microworld! 
Let's start with the basics: This is the game area. You can move around by holding middle mouse button (default) 
and moving your mouse. You can also zoom in and out by holding <Ctrl> and rotating mouse wheel (default). 
Left-click this message to continue. ]]
				, "", 3000)
			gameStop()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[This is Component Selector. If you want to place a component - that's where you get it. 
Note that if a message is pulsing with cyan, that means it has a link attached to it. 
Right-click it to open the link.]]
				, "Scene/Component Selector.edf", 3000)
			addClickableArea(0, 0, 44, windowHeight)
			setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.ComponentSelector.ComponentSelector", true)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[After you've finished creating a circuit, you can see how it behaves. To do that, use this pannel. 
You can also use hotkeys (default <F5> to start, <F6> to pause, <F7> to stop).]]
				, "Scene/Simulation Panel.edf", 3000)
			addClickableArea((windowWidth - 164) / 2, 0, 164, 43)
			setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.CircuitRunningControl", true)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[Each level already has some components on it. Let's look at the ones here.]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[This is a Power Source. It is a component that generates current.]]
				, "Components/Power Source.edf", 3000)
			addClickableComponent(PowerID)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 5) then
			showTutorial(
[[This is Ground. 
Basic principle 1: Current goes from source to ground. 
That means that in order for circuit to function, Power Source must be connected to Ground via Wires or other components.]]
				, "Components/Ground.edf", 3000)
			addClickableComponent(GroundID)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 6) then
			showTutorial(
[[This is a Light Emitting Diode (LED for short). It emmits light when powered.]]
				, "Components/LED.edf", 3000)
			addClickableComponent(LedID)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 7) then
			showTutorial(
[[Purpose of this level is to power LED for 5 seconds. Let's do that.]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 8) then
			showTutorial(
[[Wires are used to connect components. They conduct electricity between Joints, 
thus allowing components to interact with one another. 
Select Wire in Component Selector.]]
				, "Components/Wire.edf", noTimeOut)
			setComponentAvilability("Wire", -1)
			csAddClickableTile("Wire")
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 9) then
			showTutorial(
[[Most components have Joints. They are these square things. By connections Joints to one another you allow current to pass between them. 
Try connecting Power Source to LED's left Joint. To do that, simply hover your mouse over one Joint, hold down left mouse button, 
drag mouse to the other Joint, and then releases left mouse button.]]
				, "Components/Joint.edf", noTimeOut)
			addClickableComponent(PowerJointID)
			addClickableComponent(LedJointLeftID)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 10) then
			setCanStart(true)
			showTutorial(
[[Start the simulation and see what happens]]
				, "", noTimeOut)
			addClickableArea((windowWidth - 164) / 2, 0, 56, 43)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 11) then
			showTutorial(
[[Basic principle 2: Circuit must be complete in order to function. 
Right now there is no Ground connected to Power Source. Current has nowhere to go. 
Connect these two Joints to complete the circuit.]]
				, "", noTimeOut)
			addClickableComponent(LedJointRightID)
			addClickableComponent(GroundJointID)
			addClickableComponent(LedID)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 12) then
			showTutorial(
[[The circuit is now complete]]
				, "", noTimeOut)
			infoPanel = hud_infoPanel_Create(windowWidth - 350, 0, 350, 40, "Level will end in: 5");
			
			setComponentAvilability("Wire", 0)
			clearSelection()
			currentTutorial = currentTutorial + 1
		end
	end
	
	if ((lightTicks >= 0) and (getComponentBrightness(LedID) > 0.1)) then
		lightTicks = lightTicks + 1
		--setTutorialText("The circuit is now complete. Level will end in " .. tostring(5 - math.floor(lightTicks / 60)))
		hud_infoPanel_SetText(infoPanel, "Level will end in: " .. tostring(5 - math.floor(lightTicks / 60)))
		if (lightTicks >= 300) then
			completeLevel()
			lightTicks = -1
			closeTutorial()
		end
	end
end

--Called every GUI update
--eg. in menus, options, etc.
function OnGUIUpdate()
  
end

------------------------------------------------------------------------IO-------------------------------------------------------------------------

--Called when mouse button is clicked
--button==0 is left button
--button==1 is right button
--button==2 is middle button
--x,y==GUICoords
--gamex,gamey==GridCoords
function OnMouseClick(x, y, gamex, gamey, button)
  
end

------------------------------------------------------------------------GUI------------------------------------------------------------------------

--Called when a HUDScene (ex. Properties window) is added to GUIEngine
function GUI_HUDSceneOpen(name)
	if (name == "MicroWorld.Graphics.GUI.Scene.VictoryMessage") then
		clearClickableAreas()
	elseif (name == "MicroWorld.Graphics.GUI.Scene.Purpose") then
		removeHUDScenes(name)
	elseif (string.sub(name, 1, 25) == "MicroWorld.Components.GUI") then
		removeHUDScenes(name)
	end
end

--Called when a HUDScene (ex. Properties window) removed from to GUIEngine
function GUI_HUDSceneClose(name)
end

--Called when Tutorial HUDScene is clicked with left or right mouse buttons
--  left==0
-- right==1
function GUI_OnTutorialClicked(button)
end

--Called when Dialog HUDScene is clicked with left or right mouse buttons
-- left==0
-- right==1
-- middle==2
-- skip==3
function GUI_OnDialogClicked(button)
end

---------------------------------------------------------------------GLOBAL------------------------------------------------------------------------

--Called when Settings.GameState is changed.
--  Paused == 0
-- Running == 1
-- Stopped == 2
function OnGameStateChanged(old, new)
	if (currentTutorial == 11) and (new == 1) then
		closeTutorial()
	elseif (currentTutorial >= 12) then
		if (new ~= 1) then
			gameStart()
		end
	elseif (new ~= 2) then
		gameStop()
	end
end

--------------------------------------------------------------------SCRIPT_IO----------------------------------------------------------------------

--Save all information related to this scritp into "saveInfo" string
function SaveState()
	saveInfo=tostring(currentTutorial - 1)
end

--Load all script state information from state string
function LoadState(state)
	currentTutorial=tonumber(state)
end