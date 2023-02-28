function pruebaPuntoNetStatic() {
    DotNet.invokeMethodAsync("PeliculaBlazor.Client", "ObtenerCurrentCount")
        .then(resultado => {
            console.log('conteo desde javascript ' + resultado);
        })
}

function pruebaPuntoNetInstancia(dotnetHelper) {
    dotnetHelper.invokeMethodAsync("IncrementCount");
}

function timerInactivo(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        timer = setTimeout(logout, 10000 * 10) // 3 segundos
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}