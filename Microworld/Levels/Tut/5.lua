tutorialSceneName = "MicroWorld.Graphics.GUI.Scene.Tutorial"
dialogSceneName = "MicroWorld.Graphics.GUI.Scene.Dialog"

setAllComponentsUnavalable()
currentTutorial = 0
setAllowedVisibleRectangle(-400, -200, 400, 200)
setCameraScale(4)
setHistoryEnabled(false)
gameStop()
setCanStart(false)
setCanStop(false)
setCanPause(false)
setCanStep(false)

setMagneticOverlay(false)

--areaAll = getPlacableArea(0)
areaPower = getPlacableArea(0)
areaCoil = getPlacableArea(1)
areaGround = getPlacableArea(2)

idCoilTop = 8
idCoilBottom = 11
idCore = 44
--------------------------------------------------------------------COMPONENTS--------------------------------------------------------------------

--Called when SelectedComponent in ComponentSelector has changed
function OnSelectedComponentChanged(cur)
end

--Called when component is selected
function OnComponentSelected(ID)
end

--Called when a component is placed
function OnComponentPlaced(ID)
	setComponentRemovability(ID, false)
	closeTutorial()
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
clearPlacableAreas()
---------------------------------------------------------------------------------------------------------------------------------------------------
		if (currentTutorial == 0) then
			showTutorial(
[[In order to overload the Core, you will need to light up the LED. 
To do that, you will need to use elecromagnetic induction - generation of 
electric current from varying magnetic field.]]
				, "", 3000)
			--addClickableArea(1, 1, 0, 0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 1) then
			showTutorial(
[[These are Coils. They are an important part of many circuits. 
They can generate magnetic field from electric current passing through them, 
or generate electric current from the changes in magnetic field.]]
				, "Components/Coil.edf", 3000)
			addClickableComponent(idCoilTop)
			addClickableComponent(idCoilBottom)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 2) then
			showTutorial(
[[You will need another Coil to generate magnetic field. Place one here]]
				, "", noTimeOut)
			setComponentAvilability("Coil", 1)
			csAddClickableTile("Magnets/Coil")
			addPlacableArea(areaCoil[0], areaCoil[1], areaCoil[2], areaCoil[3])
			addClickablePlacableArea(0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 3) then
			showTutorial(
[[Connect the coil to the Ground]]
				, "", noTimeOut)
			setComponentAvilability("Ground", 1)
			csAddClickableTile("Basic/Ground")
			addPlacableArea(areaGround[0], areaGround[1], areaGround[2], areaGround[3])
			addClickablePlacableArea(0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 4) then
			showTutorial(
[[You will need an alternating current (AC) Pwoer Source. It differs from 
regular Power Source in that it periodically alternates the direction of the current.]]
				, "Components/AC Power Source.edf", noTimeOut)
			setComponentAvilability("AC Power Source", 1)
			csAddClickableTile("Basic/AC Power Source")
			addPlacableArea(areaPower[0], areaPower[1], areaPower[2], areaPower[3])
			addClickablePlacableArea(0)
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 5) then
			showTutorial(
[[The alternating current passing through Coil will produce varying magnetic field, that, 
when passing through the right Coils, will generate electric current to power LED. 
Close this message to complete the level]]
				, "", 10000)
			setCanStart(true)
			gameStart()
			currentTutorial = currentTutorial + 1
---------------------------------------------------------------------------------------------------------------------------------------------------
		elseif (currentTutorial == 6) then
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
		if (currentTutorial > 6) then
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
	if (currentTutorial == 6) and (new == 1) then
		closeTutorial()
		clearClickableAreas()
	else
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