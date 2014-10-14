﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOM.Dominio;
using System.Data;

namespace DOM.Interfaces
{
    public interface DAAORol
    {
        List<Rol> getAllRoles();
        Rol addRol(Rol rol);
        Rol getRol(string nombreRol);
        void updateRol(Rol rol);
        void deteleRol(string nombreRol);

        bool createRolConFuncionalidad(Rol rol, int id_funcionalidad);
        DataTable buscarRoles(string nombreRol, int id_funcionalidad);
    }
}
