tutorialSceneName = "MicroWorld.Graphics.GUI.Scene.Tutorial"
dialogSceneName = "MicroWorld.Graphics.GUI.Scene.Dialog"

setAllComponentsUnavalable()
currentTutorial = 1
setAllowedVisibleRectangle(-400, -200, 400, 200)
setCameraScale(4)
setHistoryEnabled(false)

area = getPlacableArea(0)
HES = getComponentPositionSize(4)

gameStop()
--------------------------------------------------------------------COMPONENTS--------------------------------------------------------------------

--Called when SelectedComponent in ComponentSelector has changed
function OnSelectedComponentChanged(cur)
end

--Called when component is selected
function OnComponentSelected(ID)
  
end

--Called when a component is placed
function OnComponentPlaced(ID)
	if (currentTutorial == 6) then
		closeTutorial()
		setComponentRemovability(ID, false)
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
	if (currentTutorial == 7 and isMagneticOverlay()) then
		closeTutorial()
	end
	
	if ((not isHUDSceneOpened(tutorialSceneName)) and (not isHUDSceneOpened(dialogSceneName))) then
		clearSelection()
		clearClickableAreas()
---------------------------------------------------------------------------------------------------------------------------------------------------
		if (currentTutorial == 0) then
			showTutorial(
[[]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			gameStop()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[Blue background indicates where components CAN be placed. Hatched background - where they CAN'T be placed.]]
				, "Scene/Grid.edf", 3000)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[I'm sure you're fammiliar with magnetism on some level. 
What that means here is that some components can influence other components.]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[This is Hall Effect Sensor (HES for short). It generates power based on the amount of magnetic force applied to it.
The more force it experiences, the more power it outputs.]]
				, "Components/Hall Effect Sensor.edf", 3000)
			addClickableComponent(4)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[This is a Magnet. It will influence HES. I'm sure you're familiar with it.]]
				, "Components/Magnet.edf", 3000)
			setComponentAvilability("Magnet", 1)
			csAddClickableTile("Magnets/Magnet")
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 5) then
			showTutorial(
[[Place a magnet in the highlighted area.]]
				, "", noTimeOut)
			addClickablePlacableArea(0)
			csAddClickableTile("Magnets/Magnet")
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 6) then
			showTutorial(
[[When creating circuits with magnets, magnetic overlay could come in real handy. Turn it on.]]
				, "", noTimeOut)
			addClickableArea(windowWidth - 200 + 6, 6 + 25, 200 - 12, 20)
			addClickableGameArea(area[0], area[1], HES[0] + HES[2] - area[0] + 16, 5 * 8)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 7) then
			showTutorial(
[[Magnetic overlay shows Magnet's area of effect as well as components it influences.]]
				, "", 3000)
			addClickableGameArea(area[0], area[1], HES[0] + HES[2] - area[0] + 16, 5 * 8)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 8) then
			showTutorial(
[[Start the simulation]]
				, "", noTimeOut)
			addClickableArea((windowWidth - 164) / 2, 0, 56, 43)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 9) then
--			showTutorial(
--[[End]]
--				, "", noTimeOut)
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
	if (currentTutorial == 9) then
		if (new == 1) then
			closeTutorial()
		else
			gameStop()
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