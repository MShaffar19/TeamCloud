import { getToken } from "./Auth";
import { DataResult, StatusResult, ErrorResult, Project, User } from "./model";

const scope = 'http://TeamCloud.Web/user_impersonation'
const baseUrl = 'http://localhost:3000'



export const getProject = async (id: string) => {
    return getResource<Project>(baseUrl + '/api/projects/' + id);
}

export const getProjects = async () => {
    return getResource<Array<Project>>(baseUrl + '/api/projects');
}

export const createProject = async (project: Project) => {
    return createResource(baseUrl + '/api/projects', project);
}

export const deleteProject = async (id: string) => {
    return getResource<Project>(baseUrl + '/api/projects/' + id);
}


export const getUser = async (id: string) => {
    return getResource<User>(baseUrl + '/api/users/' + id);
}

export const getUsers = async () => {
    return getResource<Array<User>>(baseUrl + '/api/users');

}

export const createUser = async (user: User) => {
    return createResource(baseUrl + '/api/users', user);
}

export const deleteUser = async (id: string) => {
    return getResource<User>(baseUrl + '/api/users/' + id);
}

export const getResource = async <T>(url: string): Promise<ErrorResult | DataResult<T>> => {

    let retry = false;

    while (true) {

        console.log("==> GET " + url);

        let response: Response = await fetch(url, {
            method: 'GET',
            mode: 'cors',
            credentials: "include",
            headers: {
                'Authorization': 'Bearer ' + await getToken(scope, retry)
            }
        });

        console.log("<== GET " + url);

        retry = response.status === 403 && !retry;

        // let json = await response.json();
        // console.log("=== JSON (" + url + ") " + JSON.stringify(json));

        if (!retry) {
            const json = await response.json();
            return response.status >= 400 ? json as ErrorResult : json as DataResult<T>;
        }
    }
}

export const deleteResource = async <T>(url: string): Promise<ErrorResult | DataResult<T>> => {

    let retry = false;

    while (true) {

        console.log("==> DELETE " + url);

        let response: Response = await fetch(url, {
            method: 'DELETE',
            mode: 'cors',
            headers: {
                'Authorization': 'Bearer ' + await getToken(scope, retry)
            }
        });

        console.log("<== DELETE " + url);

        // var json = await response.json();
        // console.log("=== JSON (" + url + ") " + JSON.stringify(json));

        retry = response.status === 403 && !retry;

        if (!retry) {
            const json = await response.json();
            return response.status >= 400 ? json as ErrorResult : json as DataResult<T>;
        }
    }
}

export const createResource = async<T>(url: string, resource: T): Promise<ErrorResult | StatusResult | DataResult<T>> => {

    let retry = false;
    let body = JSON.stringify(resource);

    while (true) {

        console.log("==> POST " + url);

        let response: Response = await fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Authorization': 'Bearer ' + await getToken(scope, retry)
            },
            body: body
        });

        console.log("<== POST " + url);

        // var json = await response.json();
        // console.log("=== JSON (" + url + ") " + JSON.stringify(json));

        retry = response.status === 403 && !retry;

        if (!retry) {
            const json = await response.json();
            return response.status === 202 ? json as StatusResult : response.status >= 400 ? json as ErrorResult : json as DataResult<T>;
        }
        // TODO: Poll status
    }
}


export const updateResource = async <T>(url: string, resource: T): Promise<DataResult<T> | StatusResult | ErrorResult> => {

    let retry = false;
    let body = JSON.stringify(resource);

    while (true) {

        console.log("==> PUT " + url);

        let response: Response = await fetch(url, {
            method: 'PUT',
            mode: 'cors',
            headers: {
                'Authorization': 'Bearer ' + await getToken(scope, retry)
            },
            body: body
        });

        console.log("<== PUT " + url);

        // var json = await response.json();
        // console.log("=== JSON (" + url + ") " + JSON.stringify(json));

        retry = response.status === 403 && !retry;

        if (!retry) {
            const json = await response.json();
            return response.status === 202 ? json as StatusResult : response.status >= 400 ? json as ErrorResult : json as DataResult<T>;
        }
        // TODO: Poll status
    }
}
