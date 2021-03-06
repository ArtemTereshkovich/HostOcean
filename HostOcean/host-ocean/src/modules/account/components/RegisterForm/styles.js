const styles = (theme) => ({
  formsLayout: {
    marginTop: "40px",
    display: "flex",
    minHeight: "100%"
  },
  paper: {
    width: 300,
    margin: "auto",
    padding: 20
  },
  error: {
    width: 200,
    color: "black"
  },
  link: {
    textDecoration: "none"
  },
  textField: {
    marginTop: 10,
    marginBottom: 10,
    width: "100%"
  },
  form: {
    display: "flex",
    flexDirection: "column"
  },
  submitButton: {
    marginTop: 10,
  },
  container: {
    position: 'relative',
    width: "100%"
  },
  suggestionsContainerOpen: {
    position: 'absolute',
    zIndex: 1,
    marginTop: theme.spacing.unit,
    left: 0,
    right: 0,
  },
  suggestion: {
    display: 'block',
  },
  suggestionsList: {
    margin: 0,
    padding: 0,
    listStyleType: 'none',
  },
  divider: {
    height: theme.spacing.unit * 2,
  },
  avatar: {
    margin: theme.spacing.unit,
    backgroundColor: theme.palette.secondary.main,
  },
  header: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center"
  },
});

export default styles;