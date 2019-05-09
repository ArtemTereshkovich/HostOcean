const styles = theme => ({
    card: {
        padding: 10,
        margin: 10,
        minWidth: 240,
        flexGrow: 1
    },
    header: {
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "space-between"
    },
    title: {
        display: "flex",
        flexDirection: "row",
        alignItems: "center",
        justifyContent: "space-between",
        marginLeft: -10
    },
    body: {
        marginTop: 10,
        display: "flex",
        flexDirection: "row",
        justifyContent: "center"
    },
    fields: {
        display: "flex",
        flexDirection: "column",
        flexGrow: 1
    },
    textField: {
        marginBottom: 10
    },
    avatar: {
        margin: theme.spacing.unit,
        backgroundColor: theme.palette.secondary.main
    },
    progressbar: {
        flexGrow: 1
    }
});

export default styles;
