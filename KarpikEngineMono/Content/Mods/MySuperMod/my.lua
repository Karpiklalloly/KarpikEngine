function on_load()
    G.log("on load called from my.lua")
end

function on_unload()
    G.log("on unload called from my.lua")
end

function on_update(dt)
    G.log("Hello from my.lua")
    G.log("dt: " .. dt)
end

function on_start()
    G.log("on start called from my.lua")
end

function on_fixed_update()
    G.log("on fixed update called from my.lua")
end