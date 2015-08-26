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
  
end

--Called when a HUDScene (ex. Properties window) removed from to GUIEngine
function GUI_HUDSceneClose(name)
  
end

--Called when Tutorial HUDScene is clicked with left or right mouse buttons
-- left==0
--right==1
function GUI_OnTutorialClicked(button)
  
end

---------------------------------------------------------------------GLOBAL------------------------------------------------------------------------

--Called when Settings.GameState is changed.
--  Paused == 0
-- Running == 1
-- Stopped == 2
function OnGameStateChanged(old, new)
  
end

--------------------------------------------------------------------SCRIPT_IO----------------------------------------------------------------------

--Save all information related to this scritp into "saveInfo" string
function SaveState()
  
end

--Load all script state information from state string
function LoadState(state)
  
end