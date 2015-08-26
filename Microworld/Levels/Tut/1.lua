tutorialSceneName = "MicroWorld.Graphics.GUI.Scene.Tutorial"
dialogSceneName = "MicroWorld.Graphics.GUI.Scene.Dialog"

setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.MapsHUD", false)

setAllComponentsUnavalable()
currentTutorial = 0
setAllowedVisibleRectangle(-400, -200, 400, 200)
setCameraScale(4)
setHistoryEnabled(false)
--------------------------------------------------------------------COMPONENTS--------------------------------------------------------------------

--Called when SelectedComponent in ComponentSelector has changed
function OnSelectedComponentChanged(cur)
end

--Called when component is selected
function OnComponentSelected(ID)
  
end

--Called when a component is placed
function OnComponentPlaced(ID)
	if (currentTutorial == 3) then
		if (areJointsConnected(0, 4)) then
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
[[Now let's look into a bit more advanced components.]]
				, "", 3000)
			gameStop()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[This is a Core. Purpose of most levels is to overload all of them. 
This kind of Core can be overloaded by powering it for a short time.
Note that Core also serves as ground.]]
				, "Components/Pulse Core.edf", 3000)
				addClickableComponent(7)
				currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[Connect these two Joints. Note, that when dragging Wire from Joint, you don't necessarily have to select it in Component Selector. 
You can have another component selected, and still place Wires.]]
				, "", noTimeOut)
			setComponentAvilability("Wire", -1)
			csAddClickableTile("Wire")
			addClickableComponent(0)
			addClickableComponent(4)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[Start the simulation]]
				, "", noTimeOut)
			addClickableArea((windowWidth - 164) / 2, 0, 56, 43)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[The Core will be overloaded shortly.]]
				, "", noTimeOut)
			setComponentAvilability("Wire", 0)
			clearSelection()
			currentTutorial = currentTutorial + 1
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
	if (currentTutorial == 4) and (new == 1) then
		closeTutorial()
	elseif (currentTutorial == 5) then
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