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
end

--Called when component is removed
function OnComponentRemoved(ID)
  
end

----------------------------------------------------------------------UPDATES----------------------------------------------------------------------

--Called every GAME update
--eg. sandbox, levels, etc
function OnGameUpdate()
	if (getSwitchState(11)) then
		if (currentTutorial == 4) then
			closeTutorial()
		elseif (currentTutorial < 4) then
			setSwitchState(11, false)
		end
	end
	if (getSwitchState(15)) then
		if (currentTutorial == 5) then
			closeTutorial()
		elseif (currentTutorial < 5) then
			setSwitchState(15, false)
		end
	end
	
	if ((not isHUDSceneOpened(tutorialSceneName)) and (not isHUDSceneOpened(dialogSceneName))) then
		clearSelection()
		clearClickableAreas()
---------------------------------------------------------------------------------------------------------------------------------------------------
		if (currentTutorial == 0) then
			showTutorial(
[[Now let's introduce you to interactive components.]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			gameStop()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[This is a Switch. It acts like a Wire, that you can connect / disconnect at any time. 
Simply click on it.]]
				, "Components/Switch.edf", 3000)
			addClickableComponent(11)
			addClickableComponent(15)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[Notice that this level has two Cores. Note that level is only considered complete when ALL cores are overloaded.]]
				, "", 3000)
			addClickableComponent(23)
			addClickableComponent(27)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[Turn on the first switch]]
				, "", noTimeOut)
			addClickableComponent(11)
			addClickableComponent(27)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[Turn on the second switch]]
				, "", noTimeOut)
			addClickableComponent(15)
			addClickableComponent(23)
			addClickableComponent(27)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 5) then
			showTutorial(
[[The level will end shortly]]
				, "", noTimeOut)
			addClickableComponent(23)
			addClickableComponent(27)
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
	if (new ~= 1) then
		gameStart()
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