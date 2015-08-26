tutorialSceneName = "MicroWorld.Graphics.GUI.Scene.Tutorial"
dialogSceneName = "MicroWorld.Graphics.GUI.Scene.Dialog"

setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.MapsHUD", false)

setAllComponentsUnavalable()
currentTutorial = 0
wireID = 0
setAllowedVisibleRectangle(-400, -200, 400, 200)
setCameraScale(4)
setHistoryEnabled(false)

corePosSize = getComponentPositionSize(36)
powerPosSize = getComponentPositionSize(28)
LEDPosSize = getComponentPositionSize(17)
photoresistorPosSize = getComponentPositionSize(44)
--------------------------------------------------------------------COMPONENTS--------------------------------------------------------------------

--Called when SelectedComponent in ComponentSelector has changed
function OnSelectedComponentChanged(cur)
end

--Called when component is selected
function OnComponentSelected(ID)
  
end

--Called when a component is placed
function OnComponentPlaced(ID)
	if (currentTutorial == 8 and areJointsConnected(11, 14)) then
		wireID = ID
		closeTutorial()
	elseif (currentTutorial == 10 and areJointsConnected(7, 14)) then
		closeTutorial()
		setComponentRemovability(ID, false)
	elseif (currentTutorial == 11 and areJointsConnected(11, 14)) then
		closeTutorial()
		setComponentRemovability(ID, false)
	else
		removeComponent(ID)
	end
end

--Called when component is removed
function OnComponentRemoved(ID)
	if (currentTutorial == 9 and ID == wireID) then
		closeTutorial()
	end
end

----------------------------------------------------------------------UPDATES----------------------------------------------------------------------

--Called every GAME update
--eg. sandbox, levels, etc
function OnGameUpdate()
	if (currentTutorial == 6 and isLuminosityOverlay()) then
		closeTutorial()
	end
	
	if ((not isHUDSceneOpened(tutorialSceneName)) and (not isHUDSceneOpened(dialogSceneName))) then
		clearSelection()
		clearClickableAreas()
---------------------------------------------------------------------------------------------------------------------------------------------------
		if (currentTutorial == 0) then
			showTutorial(
[[Let's look at more complex circuits.]]
				, "", 3000)
			addClickableArea(1, 1, 0, 0)
			gameStop()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[This is a Resistor. It's only purpose is to provide resistance.]]
				, "Components/Resistor.edf", 3000)
			addClickableComponent(9)
			addClickableComponent(13)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[Basic principle 3: The more resistance component has, the more current it consumes.]]
				, "", 3000)
			addClickableComponent(9)
			addClickableComponent(13)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[This is Photoresistor. It adjusts its resistance based on amount of light it gets (from LED). 
The more light it gets, the less resistance it has.]]
				, "Components/Photoresistor.edf", 3000)
			addClickableComponent(44)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[You often can get some usefull information about circuits behaviour with overlays.]]
				, "", 3000)
			setHUDScenesVisibility("MicroWorld.Graphics.GUI.Scene.MapsHUD", true)
			addClickableArea(windowWidth - 200, 0, 200, 57)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 5) then
			showTutorial(
[[Turn on luminosity overlay.]]
				, "", noTimeOut)
			addClickableArea(windowWidth - 200 + 6, 6, 200 - 12, 20)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 6) then
			showTutorial(
[[Luminosity overlay shows which areas of the map are lit up and by how much.]]
				, "", 3000)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 7) then
			showTutorial(
[[Let's overload that Core. Connect these Joints.]]
				, "", noTimeOut)
			setComponentAvilability("Wire", -1)
			addClickableComponent(11)
			addClickableComponent(14)
			addClickableGameArea(corePosSize[0], corePosSize[1] - 8, powerPosSize[0] + powerPosSize[2] - corePosSize[0], 5 * 8)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 8) then
			showTutorial(
[[LED's not shining bright enough - the resistance is too big. Remove the Wire (by right-clicking on it).]]
				, "", noTimeOut)
			addClickableComponent(wireID)
			addClickableGameArea(corePosSize[0], corePosSize[1] - 8, powerPosSize[0] + powerPosSize[2] - corePosSize[0], 5 * 8)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 9) then
			showTutorial(
[[Let's try the other Resistor (the one with less resistance). Connect these Joints]]
				, "", noTimeOut)
			addClickableComponent(7)
			addClickableComponent(14)
			addClickableGameArea(corePosSize[0], corePosSize[1] - 8, powerPosSize[0] + powerPosSize[2] - corePosSize[0], 5 * 8)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 10) then
			showTutorial(
[[LED is shining brighter, but still not bright enough. Connect these Joints again.]]
				, "", noTimeOut)
			addClickableComponent(11)
			addClickableComponent(14)
			addClickableGameArea(corePosSize[0], corePosSize[1] - 8, powerPosSize[0] + powerPosSize[2] - corePosSize[0], 5 * 8)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 11) then
			showTutorial(
[[Remember: Resistance of components connected paralelly (not one after another) is 
always less than resistance of each component individually.
Left-click this message to complete the level.]]
				, "", 3000)
			addClickableGameArea(corePosSize[0], corePosSize[1] - 8, powerPosSize[0] + powerPosSize[2] - corePosSize[0], 5 * 8)
			addClickableGameArea(LEDPosSize[0], LEDPosSize[1], 7 * 8, photoresistorPosSize[1] + photoresistorPosSize[3] - LEDPosSize[1])
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 12) then
			currentTutorial = currentTutorial + 1
			completeLevel()
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
		if (currentTutorial == 13) then
			clearClickableAreas()
		else
			removeHUDScenes(name)
		end
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